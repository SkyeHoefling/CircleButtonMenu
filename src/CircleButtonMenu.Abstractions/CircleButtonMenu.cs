using RealSimpleCircle.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CircleButtonMenu.Abstractions
{
    public class CircleButtonMenu : ContentView
    {
        public bool IsOpened = false;
        public Grid Grid;
        public Image RootImage;
        public List<View> Buttons;
        public CircleButtonMenu()
        {
            Buttons = new List<View>();
            Grid = new Grid
            {
                Padding = new Thickness(0, 0, 0, 0),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(0, 15, 15, 0)
            };
                       
            CreateRootButton();
            Content = Grid;
        }

        private (int TranslateX, int TranslateY) GetTranslation(int distance, Direction currentDirection)
        {
            var translateX = 0;
            var translateY = 0;

            switch (currentDirection)
            {
                case Direction.Down:
                    translateY = distance;
                    break;
                case Direction.Left:
                    translateX = -distance;
                    break;
                case Direction.Up:
                    translateY = -distance;
                    break;
                case Direction.Right:
                    translateX = distance;
                    break;
                case Direction.UpRight:
                    translateY = (int)(-distance / 1.5);
                    translateX = (int)(distance / 1.5);
                    break;
                case Direction.UpLeft:
                    translateY = (int)(-distance / 1.5);
                    translateX = (int)(-distance / 1.5);
                    break;
                case Direction.DownRight:
                    translateY = (int)(distance / 1.5);
                    translateX = (int)(distance / 1.5);
                    break;
                case Direction.DownLeft:
                    translateY = (int)(distance / 1.5);
                    translateX = (int)(-distance / 1.5);
                    break;
            }

            return (translateX, translateY);
        }

        private async void TapRootButton(object sender, EventArgs e)
        {
            if (IsOpened)
            {
                RootImage.Source = OpenImageSource;
                for (int index = 0; index < Buttons.Count; index++)
                {
                    Buttons[index].TranslateTo(0, 0);
                }
            }
            else
            {
                RootImage.Source = CloseImageSource;
                int baseDistance = 80;
                               
                for (int index = 0; index < Buttons.Count; index++)
                {
                    (int TranslateX, int TranslateY) translation;

                    if (Direction == Direction.Circle)
                    {
                        // todo - clean up redundent logic
                        if ((Direction)index == Direction.Circle || (Direction)index == Direction.Square) continue;

                        if (Flow == Flow.Snake)
                        {
                            Direction direction = Direction.Up;
                            if (index > 0)
                            {
                                direction = (Direction)index;
                            }

                            var currentButtons = Buttons.Skip(index + 1).ToList();

                            translation = GetTranslation(baseDistance, direction);
                            foreach (var current in currentButtons)
                            {
                                current.TranslateTo(translation.TranslateX, translation.TranslateY, 125);
                            }
                            
                            await Task.Delay(125);
                        }
                        else
                        {
                            var direction = (Direction)index;
                            translation = GetTranslation(baseDistance, direction);
                        }
                    }
                    else if (Direction == Direction.Square)
                    {
                        if ((Direction)index == Direction.Circle || (Direction)index == Direction.Square) continue;

                        var direction = (Direction)index;
                        var corners = new[] { Direction.UpLeft, Direction.UpRight, Direction.DownRight, Direction.DownLeft };

                        var distance = baseDistance;
                        if (corners.Contains(direction))
                        {
                            distance = (int)(distance * 1.5);
                        }

                        translation = GetTranslation(distance, direction);
                    }
                    else
                    {
                        var distance = baseDistance * (index + 1);
                        translation = GetTranslation(distance, Direction);
                    }

                    if (Flow != Flow.Snake)
                    {
                        Buttons[index].TranslateTo(translation.TranslateX, translation.TranslateY);
                    }
                }

                Grid.IsVisible = false;
                Grid.IsVisible = true;
            }

            IsOpened = !IsOpened;
        }

        private void AddButton(ImageSource image)
        {
            var newControl = new Grid();
            newControl.Children.Add(new Circle
            {
                WidthRequest = 50,
                HeightRequest = 50,
                Margin = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Active = true,
                FillColor = FillColor,
                StrokeColor = StrokeColor
            });
            newControl.Children.Add(new Image
            {
                Source = image,
                Margin = new Thickness(10)
            });

            Buttons.Add(newControl);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Command = IndexSelected;
            tapGesture.CommandParameter = Buttons.IndexOf(newControl);
            newControl.GestureRecognizers.Add(tapGesture);

            Grid.Children.Add(newControl);
        }

        public void CreateRootButton()
        {
            var newControl = new Grid();
            newControl.Children.Add(new Circle
            {
                WidthRequest = 50,
                HeightRequest = 50,
                Margin = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Active = true,
                FillColor = FillColor,
                StrokeColor = StrokeColor
            });

            RootImage = new Image
            {
                Source = OpenImageSource,
                Margin = new Thickness(10)
            };

            newControl.Children.Add(RootImage);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += TapRootButton;
            newControl.GestureRecognizers.Add(tapGesture);

            Grid.Children.Add(newControl);
        }

        public Flow Flow
        {
            get { return (Flow)GetValue(FlowProperty); }
            set { SetValue(FlowProperty, value); }
        }

        public static readonly BindableProperty FlowProperty = BindableProperty.Create(
            nameof(Flow),
            typeof(Flow),
            typeof(CircleButtonMenu),
            Flow.Expand);

        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly BindableProperty DirectionProperty = BindableProperty.Create(
            nameof(Direction),
            typeof(Direction),
            typeof(CircleButtonMenu),
            Direction.Up);

        public ICommand IndexSelected
        {
            get { return (ICommand)GetValue(IndexSelectedProperty); }
            set { SetValue(IndexSelectedProperty, value); }
        }

        public static readonly BindableProperty IndexSelectedProperty = BindableProperty.Create(
            nameof(IndexSelected),
            typeof(ICommand),
            typeof(CircleButtonMenu),
            null,
            propertyChanged: OnIndexSelectedPropertyChanged);

        private static void OnIndexSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            if (context != null)
            {
                foreach (var view in context.Buttons)
                {
                    var tapGesture = (TapGestureRecognizer)view.GestureRecognizers.FirstOrDefault();
                    tapGesture.Command = (ICommand)newValue;
                }
            }
        }

        public ImageSource OpenImageSource
        {
            get { return (ImageSource)GetValue(OpenImageSourceProperty); }
            set { SetValue(OpenImageSourceProperty, value); }
        }

        public static readonly BindableProperty OpenImageSourceProperty = BindableProperty.Create(
            nameof(OpenImageSource),
            typeof(ImageSource),
            typeof(CircleButtonMenu),
            null,
            propertyChanged: OnOpenImageSourcePropertyChanged);

        private static void OnOpenImageSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            if (context != null && context.IsOpened)
            {
                context.RootImage.Source = (ImageSource)newValue;
            }
        }

        public ImageSource CloseImageSource
        {
            get { return (ImageSource)GetValue(CloseImageSourceProperty); }
            set { SetValue(CloseImageSourceProperty, value); }
        }

        public static readonly BindableProperty CloseImageSourceProperty = BindableProperty.Create(
            nameof(CloseImageSource),
            typeof(ImageSource),
            typeof(CircleButtonMenu),
            null,
            propertyChanged: OnCloseImageSourcePropertyChanged);

        private static void OnCloseImageSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            if (context != null & !context.IsOpened)
            {
                context.RootImage.Source = (ImageSource)newValue;
            }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(CircleButtonMenu),
            null,
            propertyChanged: OnItemsSourcePropertyChanged);

        private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            var items = (IEnumerable)newValue;
            if (context != null && items != null)
            {
                context.Grid.Children.Clear();
                context.Buttons.Clear();

                foreach (var item in items)
                {
                    ImageSource source = null;
                    if (item.GetType() == typeof(string))
                    {
                        source = ImageSource.FromFile((string)item);
                    }
                    else if (item.GetType() == typeof(ImageSource))
                    {
                        source = (ImageSource)item;
                    }

                    context.AddButton(source);
                }

                context.CreateRootButton();
            }
        }

        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
            nameof(StrokeColor),
            typeof(Color),
            typeof(CircleButtonMenu),
            Color.Black,
            propertyChanged: OnStrokeColorPropertyChanged);

        private static void OnStrokeColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            var color = (Color)newValue;
            if (context != null && color != null)
            {
                foreach (var view in context.Grid.Children)
                {
                    UpdateColor(view, c => c.StrokeColor = color);
                }
            }
        }

        public Color FillColor
        {
            get { return (Color)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(
            nameof(FillColor),
            typeof(Color),
            typeof(CircleButtonMenu),
            Color.Black,
            propertyChanged: OnFillColorPropertyChanged);

        private static void OnFillColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            var color = (Color)newValue;
            if (context != null && color != null)
            {
                foreach (var view in context.Grid.Children)
                {
                    UpdateColor(view, c => c.FillColor = color);
                }
            }
        }

        private static void UpdateColor(View v, Action<Circle> updateProperties)
        {
            if (v.GetType() == typeof(Grid))
            {
                var grid = (Grid)v;
                foreach (var item in grid.Children)
                {
                    UpdateColor(item, updateProperties);
                }
            }
            else if (v.GetType() == typeof(Circle))
            {
                var circle = (Circle)v;
                updateProperties(circle);
            }
        }
    }
}
